using CoreLib;

namespace InputLib;

public class InputStage
{
    public virtual async Task Invoke(InputStage? input) => throw new NotImplementedException("Needs to be overridden");
    public virtual bool IsSuccess() => false;
    public InputStage() => Name = "IS" + Cnt++;
    public string? Name { get; set; }

    private static int Cnt = 0;

    public static readonly InputStage<InitialInputState> Initial = new() { Output = InitialInputState.Instance };

    public object? NextInput { get; set; }
}

public class InputStage<TOutput> : InputStage
{
    public InputResult<TOutput> Output { get; set; }

    //public string SimpleStageType => typeof(InputStage<TOutput>).FullName;
}

public class InputStage<TInput, TOutput> : InputStage<TOutput>
{
    public TInput Input { get; set; }

    public Func<TInput, Task<InputResult<TOutput>>> StageFunc { get; set; }

    public override async Task Invoke(InputStage? input)
    {
        if (input?.NextInput is TInput lastOutput)
        {
            Input = lastOutput;
        }
        try
        {
            var task = StageFunc(Input);
            //var awaiter = task.GetAwaiter();
            //var result = awaiter.GetResult();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }
            var result = await task;
            Output = result;
            if (result.IsSuccess(out var resResult))
            {
                NextInput = new Pair<TInput, TOutput>() { Previous = Input, Last = resResult };
            }
        }
        catch
        {
        }
    }

    public override bool IsSuccess() => Output.Success;
}