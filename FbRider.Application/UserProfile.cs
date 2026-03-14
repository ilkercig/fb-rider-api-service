namespace FbRider.Application;

public record UserProfile(string Email, string Name, ProfileImages ProfileImages);

public record ProfileImages(string Image32, string Image64, string Image128);