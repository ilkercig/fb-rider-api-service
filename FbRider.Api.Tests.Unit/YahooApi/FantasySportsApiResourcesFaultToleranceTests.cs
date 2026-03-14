using System.Xml.Serialization;
using FbRider.YahooApi;
using NUnit.Framework;

namespace FbRider.Api.Tests.Unit.YahooApi;

[TestFixture]
public class FantasySportsApiResourcesFaultToleranceTests
{
    [Test]
    public void Deserialization_ShouldBeFaultTolerant_WhenElementsAreMissing()
    {
        // Arrange
        string xml = @"<fantasy_content xmlns=""http://fantasysports.yahooapis.com/fantasy/v2/base.rng"">
    <league>
        <league_key>414.l.12345</league_key>
        <league_id>12345</league_id>
        <name>Test League</name>
        <scoring_type>headtohead</scoring_type>
    </league>
</fantasy_content>";

        XmlSerializer serializer = new XmlSerializer(typeof(FantasyContent));

        // Act
        FantasyContent? result;
        using (StringReader reader = new StringReader(xml))
        {
            result = (FantasyContent?)serializer.Deserialize(reader);
        }

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.League);
        Assert.That(result.League.LeagueKey, Is.EqualTo("414.l.12345"));
        // NumTeams is not in the XML above, it should be null or default, not throw exception
        Assert.Null(result.League.NumTeams); 
    }

    [Test]
    public void Deserialization_ShouldNotThrow_WhenRequiredPropertyIsMissing()
    {
        // Arrange - LeagueKey is 'required' in the class
        string xml = @"<fantasy_content xmlns=""http://fantasysports.yahooapis.com/fantasy/v2/base.rng"">
    <league>
        <league_id>12345</league_id>
        <name>Test League</name>
        <scoring_type>headtohead</scoring_type>
    </league>
</fantasy_content>";

        XmlSerializer serializer = new XmlSerializer(typeof(FantasyContent));

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            using (StringReader reader = new StringReader(xml))
            {
                serializer.Deserialize(reader);
            }
        });
    }
}
