using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Transitions;

[TestClass]
public class SearchTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new TransitionFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            TransitionHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);


        var query = new TransitionFieldQuery(
            search: models.First().TriggerName
        );
        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        TransitionHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Id);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithProcessIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = processId;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ProcessId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ProcessId);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithExecutorIdentityIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ExecutorIdentityId;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ExecutorIdentityId);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithActorIdentityIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ActorIdentityId;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ActorIdentityId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ActorIdentityId);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithExecutorNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ExecutorName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ExecutorName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ExecutorName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithActorNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ActorName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ActorName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ActorName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithFromActivityNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().FromActivityName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.FromActivityName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.FromActivityName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithToActivityNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ToActivityName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ToActivityName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ToActivityName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithToStateNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ToStateName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.ToStateName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ToStateName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.TransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TransitionTime);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithTransitionClassifierEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionClassifier;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.TransitionClassifier, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TransitionClassifier);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithIsFinalisedEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().IsFinalised;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.IsFinalised, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.IsFinalised);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithFromStateNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().FromStateName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.FromStateName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.FromStateName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithTriggerNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TriggerName;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.TriggerName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TriggerName);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithStartTransitionTimeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().StartTransitionTime;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.StartTransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.StartTransitionTime);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithTransitionDurationEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionDuration;

        var query = new TransitionFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TransitionField.TransitionDuration, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TransitionDuration);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var models = Enumerable.Range(0, 20).Select(_ => TransitionHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, expected.Id),
            new (FilterType.Equal, null, TransitionField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, expected.ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, expected.ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, expected.ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, expected.ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, expected.FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, expected.ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, expected.ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, expected.TransitionClassifier),
            new (FilterType.Equal, null, TransitionField.IsFinalised, expected.IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, expected.FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, expected.StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, expected.TransitionDuration),
        ];

        // Act

        var query = new TransitionFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        TransitionHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var models = Enumerable.Range(0, 20).Select(_ => TransitionHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, expected.Id),
            new (FilterType.Equal, null, TransitionField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, expected.ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, expected.ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, expected.ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, expected.ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, expected.FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, expected.ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, expected.ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, expected.TransitionClassifier),
            new (FilterType.Equal, null, TransitionField.IsFinalised, expected.IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, expected.FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, expected.StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, expected.TransitionDuration),
        ];

        // Act

        var query = new TransitionFieldQuery(filters: [new TransitionFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        TransitionHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var models = Enumerable.Range(0, 20).Select(_ => TransitionHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, models[0].Id),
            new (FilterType.Equal, null, TransitionField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, models[2].ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, models[3].ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, models[4].ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, models[5].ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, models[6].FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, models[7].ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, models[8].ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, models[9].TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, models[10].TransitionClassifier),
            // new (FilterType.Equal, null, TransitionField.IsFinalised, models[11].IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, models[11].FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, models[12].TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, models[13].StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, models[14].TransitionDuration),
        ];

        // Act

        var query = new TransitionFieldQuery(filters: [new TransitionFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            TransitionHelper.AssertModels(expectedModel, model, true);
        }
    }

    [ClientTest] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var models = Enumerable.Range(0, 20).Select(_ => TransitionHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, models[0].Id),
            new (FilterType.Equal, null, TransitionField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, models[2].ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, models[3].ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, models[4].ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, models[5].ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, models[6].FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, models[7].ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, models[8].ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, models[9].TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, models[10].TransitionClassifier),
            // new (FilterType.Equal, null, TransitionField.IsFinalised, models[11].IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, models[11].FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, models[12].TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, models[13].StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, models[14].TransitionDuration),
        ];

        var notFilters = filters.Select(f => new TransitionFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new TransitionFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            Assert.IsNull(model);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        var filter = new TransitionFieldFilter(FilterType.Equal, null, TransitionField.ProcessId, processId);
        var sort = new TransitionFieldSort(TransitionField.TriggerName, Direction.Asc);

        // Act

        var queryPage1 = new TransitionFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchProcessesTransitionsAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new TransitionFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchProcessesTransitionsAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new TransitionFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchProcessesTransitionsAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

        TransitionModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => c.Id).Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.Id == model.Id);
                TransitionHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = await TransitionHelper.ExclusiveSearchPermissionsApi(service);

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new TransitionFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesTransitionsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            TransitionHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await TransitionHelper.NoPermissionsApi(service);

        var query = new TransitionFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchProcessesTransitionsAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}