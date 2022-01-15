package discogsAlbumArt;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;


public class APIHelper {

	public static String[] DiscogsRequest(String query) throws IOException, ParseException {
		
		final String token = "rtkixvdeOIHpKwrdKBzQxQPECJzXpsmUMOAKMlig"; //from Discogs
		
		URL url = new URL("https://api.discogs.com/database/search?q=" + query + "&token=" + token);
	
		HttpURLConnection con = (HttpURLConnection) url.openConnection();
		
		con.setRequestMethod("GET");
		
		con.setRequestProperty("User-Agent", "app");
				
		BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
		
		String inputLine = "";
		
		StringBuffer content = new StringBuffer();
		
		while ((inputLine = in.readLine()) != null) {
			content.append(inputLine);
		}
				
		in.close();
		
		String jsonString = content.toString();
		
		JSONParser parser = new JSONParser();
		
		JSONObject obj_1 = (JSONObject) parser.parse(jsonString);
		
		JSONArray arr_1 = (JSONArray) obj_1.get("results");
			
		JSONObject obj_2 = (JSONObject) arr_1.get(0);
		
		String coverUrl = (String) obj_2.get("cover_image");
		
		String thumbUrl = (String) obj_2.get("thumb");
				
		return new String[] {coverUrl, thumbUrl};
	}
}
