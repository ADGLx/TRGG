﻿

public static class StaticMapConf {

    private static bool newMap = false;
    private static int size = 55; //its gotta be an odd number

    public static bool NewMap 
        {
        get
        {
            return newMap;
        }

        set
        {
            newMap = value;
        }

        }
    public static int Size
    {
        get
        {
            return size;
        }
        set
        {
            size = value;
        }
    }


}
