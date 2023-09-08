﻿using System.Text.Json;
using Xunit.Abstractions;

namespace Totoro.Plugins.Manga.MangaDex.Tests
{
    public class CatalogTests
    {
        private readonly ITestOutputHelper _output;
        private readonly JsonSerializerOptions _searializerOption = new() { WriteIndented = true };

        public CatalogTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("hyouka")]
        public async Task Search(string query)
        {
            // arrange
            var sut = new Catalog();

            // act
            var result = await sut.Search(query).ToListAsync();

            Assert.NotEmpty(result);
            foreach (var item in result)
            {
                _output.WriteLine(JsonSerializer.Serialize(item, item.GetType(), _searializerOption));
            }
        }
    }
}
