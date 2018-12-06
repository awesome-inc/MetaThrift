class UiHelper {
    static handledCall(action: () => void, couldNot: string) {
        try  {
            action(); 
        }
        catch(ex)  {
            alert(couldNot + ": " + ex.name + " " + ex.message);
        }
    }
}
