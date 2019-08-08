namespace Generation.Graphs
{

    ///Delegate for Flow
    public delegate void GenerationFlowHandler(GenerationFlow f);
    ///Delegate for Values
    [ParadoxNotion.Design.SpoofAOT]
    public delegate T ValueHandler<T>();
    ///Delegate for object casted Values only
    public delegate object ValueHandlerObject();
    ///Delegate for Flow Loop Break
    public delegate void GenerationFlowBreak();
    ///Delegate for Flow Function Return
    public delegate void GenerationReturn(object value);
}