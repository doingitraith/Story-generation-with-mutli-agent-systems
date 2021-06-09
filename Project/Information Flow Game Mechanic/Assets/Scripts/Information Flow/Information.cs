using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InformationObject
{
    NULL = -1,
    AGENT = 0,
    ITEM
}

public enum InformationVerb
{
    NULL = -1,
    IS = 0,
    HAS
}

public enum InformationProperty
{
    NULL = -1,
    ALIVE = 0,
    DEAD
}

public class Information
{
    private InformationObject _subject;
    private InformationVerb _verb;
    private InformationObject _object;
    private InformationProperty _adjective;

    public Information()
        => (_subject, _verb, _object, _adjective) =
            (InformationObject.NULL, InformationVerb.NULL, InformationObject.NULL, InformationProperty.NULL);

    /// <summary>
    /// Creates an Information of the form "Subject HAS Object" 
    /// </summary>
    /// <param name="informationSubject">Subject of the information</param>
    /// <param name="informationObject">Object of the information</param>
    public Information(InformationObject informationSubject, InformationObject informationObject)
        => (_subject, _verb, _object, _adjective) =
            (informationSubject, InformationVerb.HAS, informationObject, InformationProperty.NULL);

    /// <summary>
    /// Creates an Information of the form "Subject IS Object" 
    /// </summary>
    /// <param name="informationSubject">Subject of the information</param>
    /// <param name="informationProperty">Property of the subject</param>
    public Information(InformationObject informationSubject, InformationProperty informationProperty)
        => (_subject, _verb, _object, _adjective) =
            (informationSubject, InformationVerb.IS, InformationObject.NULL, informationProperty);
}
