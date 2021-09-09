using System;
using System.Collections.Generic;
using System.Linq;
using Information_Flow;
using NPC_Behaviour;
using Player_Behaviour;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Random = UnityEngine.Random;

namespace Game
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private DialogueRunner DialogueRunner;
        [SerializeField]
        private Text NameText;
        [SerializeField]
        private GameObject QuestionPanel;
        [SerializeField]
        private GameObject StatementPanel;
        [SerializeField]
        private GameObject Reply1;
        [SerializeField]
        private GameObject Reply2;
        [SerializeField]
        private GameObject Reply3;
        [SerializeField]
        private GameObject Reply4;
        [SerializeField]
        private Button ContinueButton;
        private Agent _currentConversationStarter;
        private Agent _currentConversationPartner;
        private Dropdown questionTerm;
        private Dropdown questionVerb;
        private Dropdown questionObject;
        private Dropdown statementSubject;
        private Dropdown statementVerb;
        private Dropdown statementObject;

        public bool IsDialogueRunning { get { return DialogueRunner.IsDialogueRunning; } }
        
        void Awake()
        {
            DialogueRunner.AddCommandHandler("ReceiveReply", ReceiveReply);
            DialogueRunner.AddCommandHandler("SetupQuestion", SetupQuestion);
            DialogueRunner.AddCommandHandler("SetupStatement", SetupStatement);
            DialogueRunner.AddCommandHandler("AskQuestion", AskQuestion);
            DialogueRunner.AddCommandHandler("MakeStatement", MakeStatement);
            questionTerm = QuestionPanel.transform.GetChild(0).GetComponent<Dropdown>();
            questionVerb = QuestionPanel.transform.GetChild(1).GetComponent<Dropdown>();
            questionObject = QuestionPanel.transform.GetChild(2).GetComponent<Dropdown>();
            statementSubject = StatementPanel.transform.GetChild(0).GetComponent<Dropdown>();
            statementVerb = StatementPanel.transform.GetChild(1).GetComponent<Dropdown>();
            statementObject = StatementPanel.transform.GetChild(2).GetComponent<Dropdown>();
        }

        public void StartDialogue(Agent conversationStarter, Agent conversationPartner)
        {
            _currentConversationStarter = conversationStarter;
            _currentConversationPartner = conversationPartner;
            
            _currentConversationStarter.Memory.TryAddNewInformation(
                new Information(_currentConversationPartner, _currentConversationPartner.Location),
                _currentConversationStarter);
            
            _currentConversationPartner.Memory.TryAddNewInformation(
                new Information(_currentConversationStarter, _currentConversationStarter.Location),
                _currentConversationPartner);
            
            
            if (_currentConversationStarter is Player)
            {
                NameText.text = _currentConversationPartner.Name;
                (_currentConversationPartner as NPC)?.InterruptNPC();
            }
            else
            {
                NameText.text = _currentConversationStarter.name;
                (_currentConversationStarter as NPC)?.InterruptNPC();
            }
            
            _currentConversationPartner.IsOccupied = true;
            _currentConversationStarter.IsOccupied = true;
            
            DialogueRunner.variableStorage.SetValue("$NPCName", NameText.text);
        
            _currentConversationPartner.CurrentReplies = _currentConversationPartner.Memory.
                    GetInformationToExchange(1, _currentConversationStarter.InformationSubject).ToList();

            DialogueRunner.variableStorage.SetValue("$HasNPCReplies", 
                _currentConversationPartner.CurrentReplies.Count!=0);
        
            if(_currentConversationPartner.CurrentReplies.Count!=0)
                DialogueRunner.variableStorage.SetValue("$NPCReplyText",
                    _currentConversationPartner.CurrentReplies[0].ToString());

            int numOfReplies = _currentConversationStarter.Memory.NumberOfMemories;
            _currentConversationStarter.CurrentReplies = _currentConversationStarter.Memory.
                    GetInformationToExchange(numOfReplies > 2 ? 3 : numOfReplies > 1 ? 2 : 1,
                        _currentConversationPartner.InformationSubject).ToList();
            
            numOfReplies = _currentConversationStarter.CurrentReplies.Count;
            DialogueRunner.variableStorage.SetValue("$NumOfReplies", numOfReplies);
            if (numOfReplies > 0)
            {
                DialogueRunner.variableStorage.SetValue("$ReplyText1", 
                    _currentConversationStarter.CurrentReplies[0].ToString());
                if(numOfReplies > 1)
                    DialogueRunner.variableStorage.SetValue("$ReplyText2", 
                        _currentConversationStarter.CurrentReplies[1].ToString());
                if(numOfReplies > 2)
                    DialogueRunner.variableStorage.SetValue("$ReplyText3", 
                        _currentConversationStarter.CurrentReplies[2].ToString());
            }
        
            DialogueRunner.StartDialogue(_currentConversationStarter.YarnNode);
        }

        public void EndDialogue()
        {
            NPC npc = null;
            if (_currentConversationStarter is Player)
                npc = _currentConversationPartner as NPC;
            else
                npc = _currentConversationStarter as NPC;
            
            _currentConversationStarter.IsOccupied = false;
            _currentConversationPartner.IsOccupied = false;

            npc.ResumeNPC();
        }

        private void ReceiveReply(string[] parameters)
        {
            var replyIdx = Int32.Parse(parameters[parameters.Length-1]);

            Agent player = null;
            Agent npc = null;
            if (_currentConversationStarter is Player)
            {
                player = _currentConversationStarter;
                npc = _currentConversationPartner;
            }
            else
            {
                player = _currentConversationPartner;
                npc = _currentConversationStarter;
            }

            if (parameters[0] == "Player")
            {
                player.Memory.TryAddNewSpeculativeInformation(npc.CurrentReplies[replyIdx], npc);
                //Debug.Log("Player learns \"" + npc.CurrentReplies[replyIdx].ToString() + "\"");
            }
            else
            {
                npc.Memory.TryAddNewSpeculativeInformation(player.CurrentReplies[replyIdx], player);
                //Debug.Log(parameters[0] + " learns \"" + player.CurrentReplies[replyIdx].ToString() + "\"");
            }
        }

        private void AskQuestion(string[] parameters)
        {
            // Hide question UI
            QuestionPanel.SetActive(false);

            // check for validity
            try
            {
                string qTerm = questionTerm.options[questionTerm.value].text;
                string qVerb = questionVerb.options[questionVerb.value].text;
                string qObject = questionObject.options[questionObject.value].text;
                
                // $IsQuestionValid 
                // $IsStatementValid
                // $QuestionQuestion
                // $QuestionAnswer
                // $StatementStatement
                // $HasAnswer
                string question = qTerm + " " + qVerb + " " + qObject+"?";
                DialogueRunner.variableStorage.SetValue("$QuestionQuestion", question);

                switch (qTerm)
                {
                    case "Who":
                    {
                        if (qVerb.Equals("Has"))
                        {
                            var answers = _currentConversationPartner.Memory.GetAllMemories()
                                .Where(c=>c.Information.Verb.ToString().Equals(qVerb) 
                                          && c.Information.Object.Name.Equals(qObject))
                                .Select(c=>c.Information.Subject.Name);
                            if (answers.Any())
                            {
                                string answer = answers.OrderBy(a => Random.value).First();
                                DialogueRunner.variableStorage.SetValue("$QuestionAnswer",
                                    "I know that "+answer+" owns "+qObject);
                                DialogueRunner.variableStorage.SetValue("$HasAnswer", true);
                            }
                            else
                                DialogueRunner.variableStorage.SetValue("$HasAnswer", false);
                        }
                        else if (qVerb.Equals("Is"))
                        {
                            var answers = _currentConversationPartner.Memory.GetAllMemories()
                                .Where(c => c.Information.Verb.ToString().Equals(qVerb)
                                            && c.Information.Adjective.ToString().Equals(qObject))
                                .Select(c => c.Information.Subject.Name);
                            if (answers.Any())
                            {
                                string answer = answers.OrderBy(a => Random.value).First();
                                DialogueRunner.variableStorage.SetValue("$QuestionAnswer",
                                    answer + " is " + qObject);
                                DialogueRunner.variableStorage.SetValue("$HasAnswer", true);
                            }
                            else
                                DialogueRunner.variableStorage.SetValue("$HasAnswer", false);
                        }
                        else
                            throw new Exception("invalid verb in AskQuestion.");
                    }
                        break;
                    case "What":
                    {
                        var answers = _currentConversationPartner.Memory.GetAllMemories()
                            .Where(c => c.Information.Verb.ToString().Equals(qVerb)
                                        && c.Information.Subject.Name.Equals(qObject))
                            .Select(c => c.Information.Object.Name);
                        if (answers.Any())
                        {
                            string answer = answers.OrderBy(a => Random.value).First();
                            DialogueRunner.variableStorage.SetValue("$QuestionAnswer",
                                "I know that " + qObject + " has " + answer);
                            DialogueRunner.variableStorage.SetValue("$HasAnswer", true);
                        }
                        else
                            DialogueRunner.variableStorage.SetValue("$HasAnswer", false);
                    }
                        break;
                    case "Where":
                    {
                        var answers = _currentConversationPartner.Memory.GetAllMemories()
                            .Where(c => c.Information.Verb.ToString().Equals(qVerb)
                                        && c.Information.Subject.Name.Equals(qObject))
                            .Select(c => c.Information.Location.Name);
                        if (answers.Any())
                        {
                            string answer = answers.OrderBy(a => Random.value).First();
                            DialogueRunner.variableStorage.SetValue("$QuestionAnswer",
                                "I know that " + qObject + " is at " + answer);
                            DialogueRunner.variableStorage.SetValue("$HasAnswer", true);
                        }
                        else
                            DialogueRunner.variableStorage.SetValue("$HasAnswer", false);
                    }
                        break;
                    case "How":
                    {
                        var answers = _currentConversationPartner.Memory.GetAllMemories()
                            .Where(c => c.Information.Verb.ToString().Equals(qVerb)
                                        && c.Information.Subject.Name.Equals(qObject))
                            .Select(c => c.Information.Adjective.ToString());
                        if (answers.Any())
                        {
                            string answer = answers.OrderBy(a => Random.value).First();
                            DialogueRunner.variableStorage.SetValue("$QuestionAnswer",
                                qObject + " is " + answer);
                            DialogueRunner.variableStorage.SetValue("$HasAnswer", true);
                        }
                        else
                            DialogueRunner.variableStorage.SetValue("$HasAnswer", false);
                    }
                        break;
                    default:
                        throw new Exception("Invalid term in AskQuestion");
                }
                
                DialogueRunner.variableStorage.SetValue("$IsQuestionValid", true);
            }
            catch (Exception e)
            {
                DialogueRunner.variableStorage.SetValue("$IsQuestionValid", false);
            }
        }
        
        private void MakeStatement(string[] parameters)
        {
            // Hide statement UI
            // form statement
            // check for validity
            throw new NotImplementedException();
        }


        public void Stop()
            => DialogueRunner.Stop();

        public void ResetDialogue()
            => DialogueRunner.ResetDialogue();

        public void AddYarnFile(YarnProgram yarnScript)
            => DialogueRunner.Add(yarnScript);
        
        private void SetupQuestion(string[] parameters)
        {
            // Show Question UI
            QuestionPanel.SetActive(true);
            Reply1.SetActive(true);
            Reply2.SetActive(false);
            Reply3.SetActive(false);
            Reply4.SetActive(false);
            questionTerm.ClearOptions();
            questionVerb.ClearOptions();
            questionObject.ClearOptions();
            
            // Fill UI elements
            var termOptions = new List<Dropdown.OptionData>
            {
                new Dropdown.OptionData("Who"),
                new Dropdown.OptionData("What"),
                new Dropdown.OptionData("Where"),
                new Dropdown.OptionData("How")
            };
            questionTerm.AddOptions(termOptions);
            OnQuestionTermValueChanged(questionTerm);
        }
        
        private void SetupStatement(string[] parameters)
        {
            // Show Statement UI
            StatementPanel.SetActive(true);
            Reply2.SetActive(false);
            Reply3.SetActive(false);
            Reply4.SetActive(false);
            statementVerb.ClearOptions();
            statementObject.ClearOptions();

            // Fill UI elements
            
        }

        public void OnQuestionTermValueChanged(Dropdown change)
        {
            questionVerb.ClearOptions();
            switch (change.options[change.value].text)
            {
                case "Who":
                {
                    var verbOptions = new List<Dropdown.OptionData>
                    {
                        new Dropdown.OptionData("Has"),
                        new Dropdown.OptionData("Is")
                    };
                    questionVerb.AddOptions(verbOptions);
                }
                    break;
                case "What":
                {
                    var verbOptions = new List<Dropdown.OptionData>
                    {
                        new Dropdown.OptionData("Has")
                    };
                    questionVerb.AddOptions(verbOptions);
                }
                    break;
                case "Where":
                {
                    var verbOptions = new List<Dropdown.OptionData>
                    {
                        new Dropdown.OptionData("At")
                    };
                    questionVerb.AddOptions(verbOptions);
                }
                    break;
                case "How":
                {
                    var verbOptions = new List<Dropdown.OptionData>
                    {
                        new Dropdown.OptionData("Is")
                    };
                    questionVerb.AddOptions(verbOptions);
                }
                    break;
                default:
                {
                    throw new Exception("No valid option selected in Question term");
                }
            }
            if(questionVerb.options.Count==1)
                OnQuestionVerbValueChanged(questionVerb);
        }
        
        public void OnQuestionVerbValueChanged(Dropdown change)
        {
            questionObject.ClearOptions();
            
            switch (change.options[change.value].text)
            {
                case "Has":
                {
                    if (questionTerm.options[questionTerm.value].text.Equals("Who"))
                    {
                        var items = _currentConversationStarter.Memory.GetKnownItems();
                        var itemOptions = new List<Dropdown.OptionData>();
                        items.ForEach(i=> itemOptions.Add(new Dropdown.OptionData(i)));
                        
                        questionObject.AddOptions(itemOptions);
                    }
                    else if(questionTerm.options[questionTerm.value].text.Equals("What"))
                    {
                        var subjects = _currentConversationStarter.Memory.GetKnownSubjects();
                        var subjectOptions = new List<Dropdown.OptionData>();
                        subjects.ForEach(i=> subjectOptions.Add(new Dropdown.OptionData(i)));
                        
                        questionObject.AddOptions(subjectOptions);
                    }
                }
                    break;
                case "At":
                {
                    var subjects = _currentConversationStarter.Memory.GetAllKnownSubjects();
                    var subjectOptions = new List<Dropdown.OptionData>();
                    subjects.ForEach(i=> subjectOptions.Add(new Dropdown.OptionData(i)));
                    
                    questionObject.AddOptions(subjectOptions);
                }
                    break;
                case "Is":
                {
                    if (questionTerm.options[questionTerm.value].text.Equals("How"))
                    {
                        var subjects = _currentConversationStarter.Memory.GetKnownSubjects();
                        var subjectOptions = new List<Dropdown.OptionData>();
                        subjects.ForEach(i=> subjectOptions.Add(new Dropdown.OptionData(i)));
                        
                        questionObject.AddOptions(subjectOptions);
                    }
                    else if(questionTerm.options[questionTerm.value].text.Equals("Who"))
                    {
                        var adjectives = _currentConversationStarter.Memory.GetKnownAdjectives();
                        var adjectiveOptions = new List<Dropdown.OptionData>();
                        adjectives.ForEach(i=> adjectiveOptions.Add(new Dropdown.OptionData(i)));
                        
                        questionObject.AddOptions(adjectiveOptions);
                    }
                }
                    break;
                default:
                    throw new Exception("Invalid Question verb selected");
            }
        }
        
        public void OnQuestionObjectValueChanged(Dropdown change)
        {
            
        }
        
        public void OnStatementSubjectValueChanged(Dropdown change)
        {
            
        }
        
        public void OnStatementVerbValueChanged(Dropdown change)
        {
            
        }
        
        public void OnStatementObjectValueChanged(Dropdown change)
        {
            
        }
    }
}
