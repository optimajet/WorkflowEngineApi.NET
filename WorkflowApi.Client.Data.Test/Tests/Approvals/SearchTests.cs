using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Approvals;

[TestClass]
public class SearchTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new ApprovalFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            ApprovalHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new ApprovalFieldQuery(
            search: models.First().IdentityId
        );

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        ApprovalHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new ApprovalFieldQuery
        {
            Filters = [new(FilterType.Equal, null, ApprovalField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

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
    public async Task ExecuteWithIdNotEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.NotEqual, null, ApprovalField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreNotEqual(expected, model.Id);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdInFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.Select(x => x.Id).Take(2).ToArray();

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.In, null, ApprovalField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= expected.Length);
        Assert.IsTrue(result.Collection.Count >= expected.Length);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(expected.Contains(model.Id));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithProcessIdEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = processId;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.ProcessId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ProcessId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdentityIdEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().IdentityId;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.IdentityId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.IdentityId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAllowedToEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().AllowedTo;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.AllowedTo, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            CollectionAssert.AreEqual(expected, model.AllowedTo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAllowedToNotEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().AllowedTo;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.NotEqual, null, ApprovalField.AllowedTo, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            CollectionAssert.AreNotEqual(expected, model.AllowedTo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAllowedToInFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.Select(x => x.AllowedTo).Take(2).ToArray();

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.In, null, ApprovalField.AllowedTo, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= expected.Length);
        Assert.IsTrue(result.Collection.Count >= expected.Length);

        foreach (var model in result.Collection)
        {
            AssertListContains(expected, model.AllowedTo);
        }
    }

    private void AssertListContains<T>(List<T>[] expected, List<T> actual)
    {
        foreach (var item in expected)
        {
            try
            {
                CollectionAssert.AreEqual(item, actual);
                return;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        Assert.Fail("No item in expected is equal to actual");
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.TransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TransitionTime);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeNotEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.NotEqual, null, ApprovalField.TransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreNotEqual(expected, model.TransitionTime);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeGreaterFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime!.Value;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Greater, null, ApprovalField.TransitionTime, expected.AddSeconds(-1))],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.TransitionTime > expected.AddSeconds(-1));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeGreaterEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime!.Value;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.GreaterEqual, null, ApprovalField.TransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.TransitionTime >= expected);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeLessFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime!.Value;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Less, null, ApprovalField.TransitionTime, expected.AddSeconds(1))],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.TransitionTime < expected.AddSeconds(1));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeLessEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TransitionTime!.Value;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.LessEqual, null, ApprovalField.TransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.TransitionTime <= expected);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTransitionTimeInFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.Select(x => x.TransitionTime).Take(2).ToArray();

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.In, null, ApprovalField.TransitionTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= expected.Length);
        Assert.IsTrue(result.Collection.Count >= expected.Length);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(expected.Contains(model.TransitionTime));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Sort;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.Sort, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Sort);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortNotEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Sort;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.NotEqual, null, ApprovalField.Sort, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreNotEqual(expected, model.Sort);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortGreaterFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Sort;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Greater, null, ApprovalField.Sort, expected - 1)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Sort > expected - 1);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortGreaterEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Sort;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.GreaterEqual, null, ApprovalField.Sort, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Sort >= expected);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortLessFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Sort;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Less, null, ApprovalField.Sort, expected + 1)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Sort < expected + 1);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortLessEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Sort;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.LessEqual, null, ApprovalField.Sort, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Sort <= expected);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSortInFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.Select(x => x.Sort).Take(2).ToArray();

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.In, null, ApprovalField.Sort, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= expected.Length);
        Assert.IsTrue(result.Collection.Count >= expected.Length);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(expected.Contains(model.Sort));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithInitialStateEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().InitialState;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.InitialState, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.InitialState);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithDestinationStateEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().DestinationState;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.DestinationState, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.DestinationState);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTriggerNameEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().TriggerName;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.TriggerName, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.TriggerName);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCommentaryEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Commentary;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ApprovalField.Commentary, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Commentary);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCommentaryNotEqualFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Commentary;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.NotEqual, null, ApprovalField.Commentary, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreNotEqual(expected, model.Commentary);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCommentaryContainsFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Commentary;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.Contains, null, ApprovalField.Commentary, expected.Substring(1, expected.Length - 2))],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Commentary.Contains(expected.Substring(1, expected.Length - 2)));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCommentaryStartsWithFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Commentary;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.StartsWith, null, ApprovalField.Commentary, expected.Substring(0, 3))],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Commentary.StartsWith(expected.Substring(0, 3)));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCommentaryEndsWithFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Commentary;

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.EndsWith, null, ApprovalField.Commentary, expected.Substring(expected.Length - 3))],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(model.Commentary.EndsWith(expected.Substring(expected.Length - 3)));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithCommentaryInFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.Select(x => x.Commentary).Take(2).ToArray();

        var query = new ApprovalFieldQuery
        {
            Filters = [new (FilterType.In, null, ApprovalField.Commentary, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= expected.Length);
        Assert.IsTrue(result.Collection.Count >= expected.Length);

        foreach (var model in result.Collection)
        {
            Assert.IsTrue(expected.Contains(model.Commentary));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var models = Enumerable.Range(0, 20).Select(_ => ApprovalHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, expected.Id),
            new (FilterType.Equal, null, ApprovalField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, expected.AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, expected.Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, expected.InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, expected.DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, expected.Commentary)
        ];

        // Act

        var query = new ApprovalFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ApprovalHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var models = Enumerable.Range(0, 20).Select(_ => ApprovalHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, expected.Id),
            new (FilterType.Equal, null, ApprovalField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, expected.AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, expected.Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, expected.InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, expected.DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, expected.Commentary)
        ];

        // Act

        var query = new ApprovalFieldQuery(filters: [new ApprovalFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ApprovalHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var models = Enumerable.Range(0, 20).Select(_ => ApprovalHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, models[0].Id),
            new (FilterType.Equal, null, ApprovalField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, models[2].IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, models[3].AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, models[4].TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, models[5].Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, models[6].InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, models[7].DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, models[8].TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, models[9].Commentary)
        ];

        // Act

        var query = new ApprovalFieldQuery(filters: [new ApprovalFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            ApprovalHelper.AssertModels(expectedModel, model, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var models = Enumerable.Range(0, 20).Select(_ => ApprovalHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, models[0].Id),
            new (FilterType.Equal, null, ApprovalField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, models[2].IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, models[3].AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, models[4].TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, models[5].Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, models[6].InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, models[7].DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, models[8].TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, models[9].Commentary)
        ];

        var notFilters = filters.Select(f => new ApprovalFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new ApprovalFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

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
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        var filter = new ApprovalFieldFilter(FilterType.Equal, null, ApprovalField.ProcessId, processId);
        var sort = new ApprovalFieldSort(ApprovalField.Id, Direction.Asc);

        // Act

        var queryPage1 = new ApprovalFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchProcessesApprovalsAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new ApprovalFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchProcessesApprovalsAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new ApprovalFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchProcessesApprovalsAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

        ApprovalModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                ApprovalHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = await ApprovalHelper.ExclusiveSearchPermissionsApi(service);

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new ApprovalFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesApprovalsAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            ApprovalHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ApprovalHelper.NoPermissionsApi(service);

        var query = new ApprovalFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchProcessesApprovalsAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}