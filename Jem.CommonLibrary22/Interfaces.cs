namespace Jem.CommonLibrary22;

/// <summary>Any class that has Id (Guid) and Name (string), where these properties can be edited.</summary>
public interface IItem : IId, IName, IIsSelectable { }

/// <summary>Any class that has Id (Guid).</summary>
public interface IId { Guid Id { get; set; } }

/// <summary>Any class that has Name (string).</summary>
public interface IName { string Name { get; set; } }

public interface IIsSelectable { bool IsSelected { get; set; } }