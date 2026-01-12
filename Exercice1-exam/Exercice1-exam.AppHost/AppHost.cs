var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Exercice1_exam_ApiServicedotnet>("apiservice");
builder.Build().Run();
