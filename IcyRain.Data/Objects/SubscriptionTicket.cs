using System;
using System.Runtime.Serialization;
using IcyRain.Tables;

namespace IcyRain.Data.Objects;

/// <summary>Запрос подписки</summary>
[DataContract, KnownType(typeof(DisconnectTicket)), KnownType(typeof(MediaLibraryTicket)),
    KnownType(typeof(UploadTrackFileData)), KnownType(typeof(UploadTracksData)), KnownType(typeof(UploadTrackReproducedsData)), KnownType(typeof(DeleteTracksData)),
    KnownType(typeof(UploadPlaylistsData)), KnownType(typeof(DeletePlaylistsData)),
    KnownType(typeof(UploadCategoriesData)), KnownType(typeof(DeleteCategoriesData))]
public abstract class SubscriptionTicket { }

/// <summary>Запрос окончания передачи данных</summary>
[DataContract]
public class DisconnectTicket : SubscriptionTicket { }

/// <summary>Запрос передачи структуры медиатеки</summary>
[DataContract]
public class MediaLibraryTicket : SubscriptionTicket { }

/// <summary>Данные обновления файла трека</summary>
[DataContract]
public class UploadTrackFileData : SubscriptionTicket
{
    [DataMember(Order = 1)]
    public Guid Guid { get; set; }

    [DataMember(Order = 2)]
    public byte[] Content { get; set; } // %%TODO Оптимизировать
}

/// <summary>Данные обновления треков</summary>
[DataContract]
public class UploadTracksData : SubscriptionTicket
{
    /// <summary>Треки</summary>
    [DataMember(Order = 1)]
    public DataTable Tracks { get; set; }
}

/// <summary>Данные обновления воспроизведения треков</summary>
[DataContract]
public class UploadTrackReproducedsData : SubscriptionTicket
{
    /// <summary>Треки</summary>
    [DataMember(Order = 1)]
    public DataTable TrackReproduceds { get; set; }
}

/// <summary>Данные удаления треков</summary>
[DataContract]
public class DeleteTracksData : SubscriptionTicket
{
    /// <summary>Треки</summary>
    [DataMember(Order = 1)]
    public Guid[] Tracks { get; set; }
}

/// <summary>Данные обновления плейлистов</summary>
[DataContract]
public class UploadPlaylistsData : SubscriptionTicket
{
    /// <summary>Плейлисты</summary>
    [DataMember(Order = 1)]
    public DataTable Playlists { get; set; }

    /// <summary>Треки в плейлистах</summary>
    [DataMember(Order = 2)]
    public DataTable PlaylistTracks { get; set; }
}

/// <summary>Данные удаления плейлистов</summary>
[DataContract]
public class DeletePlaylistsData : SubscriptionTicket
{
    /// <summary>Плейлисты</summary>
    [DataMember(Order = 1)]
    public Guid[] Playlists { get; set; }
}

/// <summary>Данные обновления категорий</summary>
[DataContract]
public class UploadCategoriesData : SubscriptionTicket
{
    /// <summary>Категории</summary>
    [DataMember(Order = 1)]
    public DataTable Categories { get; set; }
}

/// <summary>Данные удаления категорий</summary>
[DataContract]
public class DeleteCategoriesData : SubscriptionTicket
{
    /// <summary>Категории</summary>
    [DataMember(Order = 1)]
    public Guid[] Categories { get; set; }
}
