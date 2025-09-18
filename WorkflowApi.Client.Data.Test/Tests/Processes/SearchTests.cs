using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Processes;

[TestClass]
public class SearchTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new ProcessFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            ProcessHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new ProcessFieldQuery(
            search: models.First().TenantId
        );

        // Act

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        ProcessHelper.AssertModels(models.First(), actual);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.Id, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Id);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithStateNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().StateName;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.StateName, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.StateName);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithActivityNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().ActivityName;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.ActivityName, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ActivityName);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSchemeIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().SchemeId;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.SchemeId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.SchemeId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPreviousStateEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().PreviousState;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.PreviousState, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.PreviousState);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPreviousStateForDirectEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().PreviousStateForDirect;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.PreviousStateForDirect);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPreviousStateForReverseEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().PreviousStateForReverse;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.PreviousStateForReverse);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPreviousActivityEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().PreviousActivity;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.PreviousActivity, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.PreviousActivity);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPreviousActivityForDirectEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().PreviousActivityForDirect;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.PreviousActivityForDirect);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPreviousActivityForReverseEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().PreviousActivityForReverse;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.PreviousActivityForReverse);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithParentProcessIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().ParentProcessId;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.ParentProcessId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ParentProcessId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithRootProcessIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().RootProcessId;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.RootProcessId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.RootProcessId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIsDeterminingParametersChangedEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().IsDeterminingParametersChanged;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.IsDeterminingParametersChanged);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTenantIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().TenantId;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.TenantId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TenantId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithStartingTransitionEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().StartingTransition;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.StartingTransition, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.StartingTransition);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSubprocessNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().SubprocessName;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.SubprocessName, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.SubprocessName);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCreationDateEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().CreationDate;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.CreationDate, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.CreationDate);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithLastTransitionDateEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().LastTransitionDate;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.LastTransitionDate, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.LastTransitionDate);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCalendarNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().CalendarName;

        // Act

        var query = new ProcessFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ProcessField.CalendarName, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.CalendarName);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, expected.Id),
            new (FilterType.Equal, null, ProcessField.StateName, expected.StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, expected.ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, expected.SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, expected.PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, expected.PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, expected.PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, expected.PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, expected.PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, expected.PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, expected.ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, expected.IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, expected.TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, expected.StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, expected.SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, expected.CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, expected.LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, expected.CalendarName),
        ];

        // Act

        var query = new ProcessFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ProcessHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, expected.Id),
            new (FilterType.Equal, null, ProcessField.StateName, expected.StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, expected.ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, expected.SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, expected.PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, expected.PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, expected.PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, expected.PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, expected.PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, expected.PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, expected.ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, expected.IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, expected.TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, expected.StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, expected.SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, expected.CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, expected.LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, expected.CalendarName),
        ];

        // Act

        var query = new ProcessFieldQuery(filters: [new ProcessFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ProcessHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, models[0].Id),
            new (FilterType.Equal, null, ProcessField.StateName, models[1].StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, models[2].ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, models[3].SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, models[4].PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, models[5].PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, models[6].PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, models[7].PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, models[8].PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, models[9].PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, models[10].ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, models[11].RootProcessId),
            // new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, models[12].IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, models[12].TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, models[13].StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, models[14].SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, models[15].CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, models[16].LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, models[17].CalendarName),
        ];

        // Act

        var query = new ProcessFieldQuery(filters: [new ProcessFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            ProcessHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, models[0].Id),
            new (FilterType.Equal, null, ProcessField.StateName, models[1].StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, models[2].ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, models[3].SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, models[4].PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, models[5].PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, models[6].PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, models[7].PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, models[8].PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, models[9].PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, models[10].ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, models[11].RootProcessId),
            // new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, models[12].IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, models[12].TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, models[13].StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, models[14].SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, models[15].CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, models[16].LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, models[17].CalendarName),
        ];

        var notFilters = filters.Select(f => new ProcessFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new ProcessFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            Assert.IsNull(model);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(30);

        await repository.CreateAsync(models);

        var filter = new ProcessFieldFilter(FilterType.In, null, ProcessField.Id, models.Select(m => m.Id));
        var sort = new ProcessFieldSort(ProcessField.Id, Direction.Asc);

        // Act

        var queryPage1 = new ProcessFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchProcessesAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new ProcessFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchProcessesAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new ProcessFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchProcessesAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

        ProcessModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                ProcessHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = await ProcessHelper.ExclusiveSearchPermissionsApi(service);

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new ProcessFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            ProcessHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ProcessHelper.NoPermissionsApi(service);

        var query = new ProcessFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchProcessesAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}