package discogsAlbumArt;

import java.io.IOException;

import org.json.simple.parser.ParseException;

public class program {

	public static void main(String[] args) {
		// TODO Auto-generated method stub

		try {
			APIHelper.DiscogsRequest("The White Stripes");
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

}
