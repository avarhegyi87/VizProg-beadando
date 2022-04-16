CREATE VIEW [dbo].BandDetails
	AS SELECT 
		t.[Band Name],
		t.Genre,
		t.[Founded in],
		t.[No of albums],
		t.[Avg album rating],
		t.[No of members]
	FROM(
		SELECT TOP 10 
			b.band_id AS 'Band Id',
			b.band_name AS 'Band Name',
			b.genre_name AS 'Genre',
			b.date_founding AS 'Founded in',
			COUNT(a.album_id) AS 'No of albums',
			ROUND(AVG(a.album_rating),1) AS 'Avg album rating',
			COUNT(m.musician_id) AS 'No of members'
		FROM dbo.MetalBands AS b
		LEFT JOIN dbo.Albums AS a ON b.band_id = a.band_id
		LEFT JOIN dbo.BandMembers AS m ON a.band_id = m.band_id
		GROUP BY b.band_id, b.band_name, b.genre_name, b.date_founding
		ORDER BY AVG(a.album_rating) DESC) AS t
