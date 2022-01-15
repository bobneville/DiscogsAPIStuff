package discogsAlbumArt;

import java.net.*;

public class Album {

	public int Number;
	public int Year;
	public String Album;
	public String Artist;
	public String Genre;
	public String SubGenre;
	public String ThumbnailUrl;
	public String CoverUrl;
	
	public void Build(String[] input) {
		Number = Integer.parseInt(input[0]);
		Year = Integer.parseInt(input[1]);
		Album = input[2];
		Artist = input[3];
		Genre = input[4];
		SubGenre = input[5];
		ThumbnailUrl = input[6];
		CoverUrl = input[7];
		
		try {
			String[] result = APIHelper.DiscogsRequest(String.format("{0} {1}",Album, Artist));
			ThumbnailUrl = result[0];
			CoverUrl = result[1];
		}
		catch(Exception e) {
			System.out.println("Failed http request");
		}
	}
	
	@Override
	public String toString() {
		return String.format("{0} {1} {2} {3} {4} {5} {6} {7}", Number, Year, Album, Artist, Genre, SubGenre, ThumbnailUrl, CoverUrl );
	}
}
