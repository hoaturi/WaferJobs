﻿using MessagePack;

namespace JobBoard.Infrastructure.Services.LookupServices.PopularKeywordsService;

[MessagePackObject]
public record PopularKeywordDto(
    [property: Key(0)] string Keyword,
    [property: Key(1)] int Count
);