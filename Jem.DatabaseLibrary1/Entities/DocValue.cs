﻿public sealed class DocValue : IdNamed
{
    public Guid? OwnerDocumentId { get; set; }
    public DocDocument? OwnerDocument { get; set; }

    [MaxLength(2000)]
    public string? Value { get; set; }

    public string? Source { get; set; }
    public int? SourceIndex { get; set; }
    public double? SourceLeft { get; set; }
    public double? SourceTop { get; set; }
    public double? SourceWidth { get; set; }
    public double? SourceHeight { get; set; }

}