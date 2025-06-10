using Unity.VisualScripting;

public class Utils { 
   public static float Par (float HP, float MaxHP)
    {
        return HP != 0 && MaxHP != 0? HP / MaxHP : 0;
    }
}
