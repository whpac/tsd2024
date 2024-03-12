import java.util.Collection;
import java.util.Comparator;
import java.util.Date;
import java.util.List;
import java.util.stream.Collectors;

public class JavaStream {
    public static void main(String[] args) {
        System.out.println("Hello world!");

        Collection<GoldPrice> prices = null;
        List<GoldPrice> top3 = prices.stream()
                .sorted(Comparator.comparing(a -> a.Date))
                .limit(3)
                .collect(Collectors.toList());
    }

    class GoldPrice {
        public Date Date;
        public double Price;
    }
}