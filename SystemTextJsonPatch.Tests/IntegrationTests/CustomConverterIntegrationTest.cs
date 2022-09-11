﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Converters;
using SystemTextJsonPatch.Operations;
using Xunit;

namespace SystemTextJsonPatch.Tests.IntegrationTests
{
    public class CustomConverterIntegrationTest
    {
        [Fact]
        public void CopyUsesCustomConvertersForSerialization()
        {
            // Arrange
            var targetObject = new TesterObject()
            {
                MyDate = new DateOnly(2020, 1, 1)
            };

            var patchDocument = new JsonPatchDocument(new List<Operation>(), new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonPatchDocumentConverterFactory(),
                    new DateOnlyConverter()
                }
            });
            patchDocument.Copy("MyDate", "AnotherDate");

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(targetObject.MyDate, targetObject.AnotherDate);
        }

        [Fact]
        public void CopyUsesCustomConvertersForSerializationWhenDeserializingJsonPatch()
        {
            // Arrange
            var targetObject = new TesterObject()
            {
                MyDate = new DateOnly(2020, 1, 1)
            };

            var options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonPatchDocumentConverterFactory(),
                    new DateOnlyConverter()
                },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };


            var doc = new JsonPatchDocument(new List<Operation>()
            {
                new Operation("replace", "myDate", null, "2021-02-02"),
                new Operation("copy", "anotherDate", "myDate")
            }, options);


            var docJson = JsonSerializer.Serialize(doc, options);
            var patchDoc =
                JsonSerializer.Deserialize<JsonPatchDocument<TesterObject>>(docJson, options);

            patchDoc.ApplyTo(targetObject);

            Assert.Equal(new DateOnly(2021, 2, 2), targetObject.AnotherDate);
        }
    }

    public class TesterObject
    {
        public DateOnly MyDate { get; set; }

        public DateOnly AnotherDate { get; set; }
    }


    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;

        public DateOnlyConverter() : this(null)
        {
        }

        public DateOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader,
            Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value,
            JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(serializationFormat));
    }
}
