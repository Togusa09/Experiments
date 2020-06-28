using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests
{
    public class VoxelCoorindateTest
    {
        VoxelCoordinateCalculator _voxelCoordinateCalculator;

        [SetUp]
        public void Setup()
        {
            _voxelCoordinateCalculator = new VoxelCoordinateCalculator(10);
        }

        [Test]
        public void TestDefaultCase()
        {
            var voxelCoords = _voxelCoordinateCalculator.CalculateId(Vector3.zero);

            Assert.AreEqual(voxelCoords.IdString, "0|0|0");
            Assert.AreEqual(voxelCoords.VoxelGroupPosition, Vector3Int.zero);
            Assert.AreEqual(voxelCoords.VoxelLocalPosition, Vector3Int.zero);
        }

        [Test]
        public void TestIntCoordInCenterGroup()
        {
            var voxelCoords = _voxelCoordinateCalculator.CalculateId(new Vector3(1, 2, 3));

            Assert.AreEqual(voxelCoords.IdString, "0|0|0");
            Assert.AreEqual(voxelCoords.VoxelGroupPosition, Vector3Int.zero);
            Assert.AreEqual(voxelCoords.VoxelLocalPosition, new Vector3Int(1, 2, 3));
        }

        [Test]
        public void TestFloatCoordInCenterGroup()
        {
            var voxelCoords = _voxelCoordinateCalculator.CalculateId(new Vector3(1.1f, 2.2f, 3.3f));

            Assert.AreEqual(voxelCoords.IdString, "0|0|0");
            Assert.AreEqual(voxelCoords.VoxelGroupPosition, Vector3Int.zero);
            Assert.AreEqual(voxelCoords.VoxelLocalPosition, new Vector3Int(1, 2, 3));
        }


        [Test]
        public void TestFloatCoordInNonCenterGroup()
        {
            var voxelCoords = _voxelCoordinateCalculator.CalculateId(new Vector3(11.1f, 12.2f, 13.3f));

            Assert.AreEqual("1|1|1", voxelCoords.IdString);
            Assert.AreEqual(new Vector3Int(10, 10, 10), voxelCoords.VoxelGroupPosition);
            Assert.AreEqual(new Vector3Int(1, 2, 3), voxelCoords.VoxelLocalPosition);
        }

        [Test]
        public void TestFloatCoordInNegativeNonCenterGroup()
        {
            var voxelCoords = _voxelCoordinateCalculator.CalculateId(new Vector3(-1.1f, -2.2f, -3.3f));

            Assert.AreEqual("-1|-1|-1", voxelCoords.IdString);
            Assert.AreEqual(new Vector3Int(-10, -10, -10), voxelCoords.VoxelGroupPosition);
            Assert.AreEqual(new Vector3Int(8, 7, 6), voxelCoords.VoxelLocalPosition);

            var voxelCoords2 = _voxelCoordinateCalculator.CalculateId(new Vector3(-11.1f, -12.2f, -13.3f));

            Assert.AreEqual("-2|-2|-2", voxelCoords2.IdString);
            Assert.AreEqual(new Vector3Int(-20, -20, -20), voxelCoords2.VoxelGroupPosition);
            Assert.AreEqual(new Vector3Int(8, 7, 6), voxelCoords2.VoxelLocalPosition);

            var voxelCoords3 = _voxelCoordinateCalculator.CalculateId(new Vector3(-0.5f, 0, 0));

            Assert.AreEqual("-1|0|0", voxelCoords3.IdString);
            Assert.AreEqual(new Vector3Int(-10, 0, 0), voxelCoords3.VoxelGroupPosition);
            Assert.AreEqual(new Vector3Int(9, 0, 0), voxelCoords3.VoxelLocalPosition);

            var voxelCoords4 = _voxelCoordinateCalculator.CalculateId(new Vector3(-9.9f, 0, 0));

            Assert.AreEqual("-1|0|0", voxelCoords4.IdString);
            Assert.AreEqual(new Vector3Int(-10, 0, 0), voxelCoords4.VoxelGroupPosition);
            Assert.AreEqual(new Vector3Int(0, 0, 0), voxelCoords4.VoxelLocalPosition);

            var voxelCoords5 = _voxelCoordinateCalculator.CalculateId(new Vector3(-10f, 0, 0));

            Assert.AreEqual("-1|0|0", voxelCoords5.IdString);
            Assert.AreEqual(new Vector3Int(-10, 0, 0), voxelCoords5.VoxelGroupPosition);
            Assert.AreEqual(new Vector3Int(0, 0, 0), voxelCoords5.VoxelLocalPosition);
        }
    }
}
