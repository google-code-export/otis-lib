using System;
using System.Collections.Generic;
using System.Text;
using Otis.Sample.Domain;

namespace Otis.Sample.Presentation
{
	public class RecordInfo
	{
		private int m_id;
		private int m_songCount;
		private int m_commentCount;
		private string m_description;
		private string m_averageSongDuration;
		private int m_albumDuration;
		private double m_averageRating;
		private string m_albumDurationText = "";
		private MusicCategory m_category = MusicCategory.Undefined;

		public int Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public int SongCount
		{
			get { return m_songCount; }
			set { m_songCount = value; }
		}

		public string Description
		{
			get { return m_description; }
			set { m_description = value; }
		}

		public int TotalCommentCount
		{
			get { return m_commentCount; }
			set { m_commentCount = value; }
		}

		public double AverageSongRating
		{
			get { return m_averageRating; }
			set { m_averageRating = value; }
		}

		public string AverageSongDuration
		{
			get { return m_averageSongDuration; }
			set { m_averageSongDuration = value; }
		}

		public int AlbumDuration
		{
			get { return m_albumDuration; }
			set { m_albumDuration = value; }
		}

		public string AlbumDurationText
		{
			get { return m_albumDurationText; }
		}

		public MusicCategory Category
		{
			get { return m_category; }
			set { m_category = value; }
		} 
		
		// this function is defined as mapping helper in mapping xml file
		// it is called after the assembler processes the source object
		// you can provide your own custom behavior here
		public void ConversionHelper(ref RecordInfo target, ref Record source)
		{
			// conver this to more friendly format
			target.m_albumDurationText = string.Format("{0}:{1:00}", AlbumDuration / 60, AlbumDuration % 60);
		}
	}
}
