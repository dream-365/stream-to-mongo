# Quick Start

## Pipeline

define data inputs and data outputs in a json file: _local_model_sotre_.json.

```
var pipeline = new Pipeline { BoundedCapacity = 10 };
var source = new List<string>
{
    "message#1",
    "message#2",
    "message#3"
};
var result = new List<string>();
var target = new ListWriter<string>(result);
pipeline.Read(source)
        .Transform(item => item.Replace("#", "@"))
        .Write(target);
var exe = pipeline.Build();
var runningResult = exe.RunAsync().Result;
```

## Comparing Stage