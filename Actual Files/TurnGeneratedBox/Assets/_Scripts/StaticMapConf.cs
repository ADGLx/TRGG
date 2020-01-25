

public static class StaticMapConf {

    private static bool newMap = false;
    private static int size = 55; //This gotta change when the map becomes infinite

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
