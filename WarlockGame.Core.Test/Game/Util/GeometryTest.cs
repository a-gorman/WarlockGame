using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Util;
using Xunit;

namespace WarlockGame.Core.Test.Game.Util;

[TestSubject(typeof(Geometry))]
public class GeometryTest {
    [Fact]
    public void CreateRectangle_CreatesCorrectly() {
        var result = Geometry.CreatePolygonFromRectangle(new Vector2(10, 10), 3, 5);

        result.Vertices.Should()
            .HaveCount(4)
            .And
            .Contain([
                new Vector2(11.5f, 12.5f),
                new Vector2(11.5f, 7.5f),
                new Vector2(8.5f,  12.5f),
                new Vector2(8.5f,  7.5f)
            ]);
    }
    
    [Fact]
    public void GetSides_CreatesSidesClockwise() {
        var rect = new OrientedRectangle(new Vector2(10, 20), new SizeF(3, 5), Matrix3x2.CreateRotationZ(float.Pi / 3));

        var result = rect.GetSides();
        
        result.Should().HaveCount(4);
        for (int i = 0; i < 4; i++) {
            result[i].End.Should().Be(result[i == 3 ? 0 : i + 1].Start);
        }
    }
}