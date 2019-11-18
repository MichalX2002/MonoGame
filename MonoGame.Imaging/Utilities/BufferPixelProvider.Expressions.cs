
private delegate void PixelConversionDelegate<TConverter>(
    ref TConverter convert, int toRead, ref int bufferOffset);

static PixelConversionDelegate<TConverter> CreateConversionDelegate<TConverter>(
    string dataFieldName, string rawDataFieldName, int elementSize)
{
    var convertParam = Expression.Parameter(typeof(TConverter).MakeByRefType());
    var toReadParam = Expression.Parameter(typeof(int));
    var bufferOffsetParam = Expression.Parameter(typeof(int).MakeByRefType());

    var dataField = Expression.Field(convertParam, dataFieldName);
    var rawDataField = Expression.Field(convertParam, rawDataFieldName);
    var elementSizeConstant = Expression.Constant(elementSize);

    var loopVariable = Expression.Variable(typeof(int));
    var assignLoopVariable = Expression.Assign(loopVariable, Expression.Constant(0));
    var loopCondition = Expression.LessThan(loopVariable, toReadParam);
    var loopConditionNot = Expression.Not(loopCondition);
    var breakLabel = Expression.Label();
    var loopBody = Expression.Block(
        Expression.IfThen(loopConditionNot, Expression.Break(breakLabel)),
        //bruh do them conversions,
        Expression.PostIncrementAssign(loopVariable),
        Expression.AddAssignChecked(bufferOffsetParam, elementSizeConstant)
        );
    var loop = Expression.Loop(loopBody, breakLabel);

    var copyRest = Expression.Call();
    var lambdaBody = Expression.Block(new[] { loopVariable }, assignLoopVariable, loop, copyRest);

    var lambdaExpression = Expression.Lambda<PixelConversionDelegate<TConverter>>(
        lambdaBody, convertParam, toReadParam, bufferOffsetParam);

    var compiledLambda = lambdaExpression.Compile();
    return compiledLambda;
}