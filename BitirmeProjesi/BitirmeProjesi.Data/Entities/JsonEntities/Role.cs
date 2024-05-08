using System.Text.Json.Serialization;

namespace BitirmeProjesi.Data.Entities.JsonEntities;

// Kullanıcıya atayabileceğimiz roller 
public enum Role { Admin, User }

// Admin için 0 User için 1 seçilmelidir
