using Rhino.Geometry;
using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.Convert.Geometry
{
  static class MeshDecoder
  {
    internal static Mesh ToRhino(DB.Mesh mesh)
    {
      return Raw.RawDecoder.ToRhino(mesh);
    }

    internal static Mesh FromRawMesh(Mesh mesh, double scaleFactor)
    {
      if (scaleFactor != 1.0 && !mesh.Scale(scaleFactor))
        return default;

      if (!mesh.IsValidWithLog(out var log))
      {
        if (log.Contains("has degenerate double precision vertex locations"))
        {
          var fixedFaceCount = 0;
          mesh.Faces.RemoveZeroAreaFaces(ref fixedFaceCount);
        }
      }

      mesh.Ngons.AddPlanarNgons(Revit.VertexTolerance, 4, 2, true);
      return mesh;
    }
  }
}
