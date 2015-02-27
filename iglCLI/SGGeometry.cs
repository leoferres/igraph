using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGraph.StatGraph {
  public class SGGeometry {

    public SGGeometry(double posx, double posy, double height, double width) {
      this.PosX = posx;
      this.PosY = posy;
      this.Height = height;
      this.Width = width;
    }

    public double PosX {
      get;
      set;
    }

    public double PosY {
      get;
      set;
    }

    public double Height {
      get;
      set;
    }

    public double Width {
      get;
      set;
    }

    public string showAsString() {
      
      string s = String.Format("posX={0}, posY={1}, h={2}, w={3}", 
        this.PosX, this.PosY, this.Height, this.Width);
      
      return s;
    }
  }
}
