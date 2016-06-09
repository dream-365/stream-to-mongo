# Quick Start

## Data inputs and outputs

define data inputs and data outputs in a json file: _local_model_sotre_.json.

```
{
  "inputs": {
    "office_365_threads": {
      "sourceType": "mongo",
      "objectName": "landing.office_365_threads",
      "connection": "mongodb://localhost:27017",
      "filter": "{}"
    }
  },
  "outputs": {
    "output_test": {
      "targetType": "mongo",
      "connection": "mongodb://localhost:27017",
      "objectName": "landing.output_test",
      "columnNames": [ "_id", "questionId", "title", "createdOn", "createdBy" ],
      "isUpsert": true,
      "primaryKey": "_id"
    }
  }
}
```

## Data processing

define a transform action to process the data row

```
    public class QuestionBasicTransformAction : BaseTransformAction
    {
        protected override void Build()
        {
            Copy("_id", "questionId");

            Copy("authorId", "createdBy");
        }
    }
```

define a transform package to compose input, action and ouput
```
    public class Package1 : PackageInstallation
    {
        public override void OnCreate(PackageBuilder builder)
        {
            builder.Input("office_365_threads")
                   .Action(new QuestionBasicTransformAction())
                   .Output("output_test");
        }
    }
```

## Install and run transform package
```
    static void Main(string[] args)
    {
        LemonTransform.UseDefaultSevices();

        LemonTransform.InstallPackage<Package1>("package1");

        var engine = new CoreDocumentTransformEngine();

        engine.Execute("package1");
    }
```
