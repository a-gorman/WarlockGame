using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game;

namespace WarlockGame.Core.Test;

public class ExtensionsTests
{
    [Fact]
    public void SubdivideRectangle_doesSubdivide_atOrigin()
    {
        List<Rectangle> result = new Rectangle(0, 0, 10, 20).Subdivide(2, 2).ToList();

        result.Should().HaveCount(4);
        result.Skip(0).First().Should().BeEquivalentTo(new Rectangle(0, 0, 5, 10));
        result.Skip(1).First().Should().BeEquivalentTo(new Rectangle(0, 10, 5, 10));
        result.Skip(2).First().Should().BeEquivalentTo(new Rectangle(5, 0, 5, 10));
        result.Skip(3).First().Should().BeEquivalentTo(new Rectangle(5, 10, 5, 10));
    }
}