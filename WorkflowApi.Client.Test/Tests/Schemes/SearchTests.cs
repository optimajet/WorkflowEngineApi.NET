using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Models;
using WorkflowApi.Client.Test.Runner;
using Direction = WorkflowApi.Client.Model.Direction;
using FilterType = WorkflowApi.Client.Model.FilterType;
using Scheme = WorkflowApi.Client.Test.Models.Scheme;
using SchemeField = WorkflowApi.Client.Model.SchemeField;

namespace WorkflowApi.Client.Test.Tests.Schemes;

[TestClass]
public class SearchTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new SchemeFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Code == models[i].Code);
            SchemeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new SchemeFieldQuery(
            search: models.First().Code
        );

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        SchemeHelper.AssertModels(models.First(), actual);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithCodeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Code;

        var query = new SchemeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, SchemeField.Code, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Code);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSchemeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Scheme;

        var query = new SchemeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, SchemeField.Scheme, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            var expectedSchemaString = Converter.ToSchemaString(
                JsonConvert.DeserializeObject<Scheme>(JsonConvert.SerializeObject(expected))
                ?? throw new InvalidOperationException("Failed to deserialize scheme from model.")
            );

            var actualSchemaString = Converter.ToSchemaString(
                JsonConvert.DeserializeObject<Scheme>(JsonConvert.SerializeObject(model.Scheme))
                ?? throw new InvalidOperationException("Failed to deserialize scheme from model.")
            );

            Assert.AreEqual(expectedSchemaString, actualSchemaString);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithCanBeInlinedEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().CanBeInlined;

        var query = new SchemeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, SchemeField.CanBeInlined, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.CanBeInlined);
        }
    }

    [ClientTest(ProviderName.Mongo)] // Mongo does not support collection equality
    [TestMethod]
    public async Task ExecuteWithInlinedSchemesEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().InlinedSchemes;

        var query = new SchemeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, SchemeField.InlinedSchemes, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            CollectionAssert.AreEqual(expected, model.InlinedSchemes);
        }
    }

    [ClientTest(ProviderName.Mongo)] // Mongo does not support collection equality
    [TestMethod]
    public async Task ExecuteWithTagsEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Tags;

        var query = new SchemeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, SchemeField.Tags, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            CollectionAssert.AreEqual(Converter.SimplifyTags(expected), Converter.SimplifyTags(model.Tags));
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, expected.Code),
            new (FilterType.Equal, null, SchemeField.Scheme, expected.Scheme),
            new (FilterType.Equal, null, SchemeField.CanBeInlined, expected.CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, expected.InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, expected.Tags),
        ];

        // Act

        var query = new SchemeFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        SchemeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, expected.Code),
            new (FilterType.Equal, null, SchemeField.Scheme, expected.Scheme),
            new (FilterType.Equal, null, SchemeField.CanBeInlined, expected.CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, expected.InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, expected.Tags),
        ];

        // Act

        var query = new SchemeFieldQuery(filters: [new SchemeFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        SchemeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, models[0].Code),
            new (FilterType.Equal, null, SchemeField.Scheme, models[1].Scheme),
            // new (FilterType.Equal, null, SchemeField.CanBeInlined, models[2].CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, models[2].InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, models[3].Tags),
        ];

        // Act

        var query = new SchemeFieldQuery(filters: [new SchemeFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Code == model.Code);
            SchemeHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, models[0].Code),
            new (FilterType.Equal, null, SchemeField.Scheme, models[1].Scheme),
            // new (FilterType.Equal, null, SchemeField.CanBeInlined, models[2].CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, models[2].InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, models[3].Tags),
        ];

        var notFilters = filters.Select(f => new SchemeFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new SchemeFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Code == models[i].Code);
            Assert.IsNull(model);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(30);

        await repository.CreateAsync(models);

        var filter = new SchemeFieldFilter(FilterType.In, null, SchemeField.Code, models.Select(m => m.Code));
        var sort = new SchemeFieldSort(SchemeField.Code, Direction.Asc);

        // Act

        var queryPage1 = new SchemeFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchSchemesAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new SchemeFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchSchemesAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new SchemeFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchSchemesAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

        SchemeModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => c.Code).Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.Code == model.Code);
                SchemeHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusiveSearchPermissionsApi(service);

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new SchemeFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchSchemesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Code == models[i].Code);
            SchemeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await SchemeHelper.NoPermissionsApi(service);

        var query = new SchemeFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchSchemesAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}