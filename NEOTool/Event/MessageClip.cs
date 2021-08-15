using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using NEOTool.Text;
namespace NEOTool.Event
{
  public class MessageClip : IEquatable<MessageClip>
  {
    public long AssetId { get; }
    public string TextId;
    public TextEntry GameText { get; }
    
    public string Text => GameText.English;
    private bool HasPose;
    private string PoseId;
    private bool HasFace;
    private string FaceId;
    private string DebugClipPath;

    public MessageClip(String clipPath, BinaryReader reader, Expressions expressions, GameText gameText, Speakers speakers)
    {
      DebugClipPath = clipPath;
      reader.ReadBytes(0x14); // Unknown.
      AssetId = reader.ReadInt64();
      ReadString(reader); // Asset name. We don't need this.
      ReadString(reader); // Unknown string.
      reader.ReadBytes(0x1C); // Unknown.
      ReadString(reader); // Unknown. "Small" in one of the boss line files. Probably has to do with pre-made size configs for boss stuff.
      TextId = ReadString(reader);
      reader.ReadInt32(); // Unknown.
      // If this is 0, we need to halt here.
      var flag = reader.ReadInt32();
      long poseFileId = 0;
      long faceFileId = 0;
      if (flag != 0)
      {
        ReadString(reader); // Should be "pose."
        reader.ReadInt32();
        poseFileId = reader.ReadInt64();
        // If there's no face, we can bail here.
        reader.ReadInt32();
        try
        {
          ReadString(reader); // Should be "face."
          reader.ReadInt32();
          faceFileId = reader.ReadInt64();
          HasFace = faceFileId != 0 && faceFileId != poseFileId;
          // We're done now, the rest of the file is irrelevant.
        }
        catch (IOException)
        {
          // Just interrupt reading if we get an IOException. Godsdamn, I wish that "hasFace" flag actually worked.
        }
      }

      // TODO: Fret's got poses that don't have a face by default—need to see if other characters have a similar thing, and if so, document them
      // TODO: Gods, just rewrite this shit, look at how BAD it is
      if (TextId != string.Empty && gameText.ContainsKey(TextId))
      {
        GameText = gameText[TextId];
        GameText.FriendlySpeaker = speakers[GameText.Speaker];
        if (poseFileId == 0 && faceFileId == 0)
        {
          HasPose = false;
          HasFace = false;
        }
        else
        {
          if (expressions.Poses.ContainsKey(GameText.FriendlySpeaker))
          {
            if (expressions.Poses[GameText.FriendlySpeaker].ContainsKey(poseFileId))
            {
              PoseId = expressions.Poses[GameText.FriendlySpeaker][poseFileId];
              if (HasFace)
              {
                /// This one's a real headache, but there's a clip of Shiba where his face is set to what is normally a basic pose file ID. If something
                /// like that happens, then we need to check the poses and see if that ID exists there. If so, override the pose ID.
                if (expressions.Faces[GameText.FriendlySpeaker][PoseId].ContainsKey(faceFileId) == false)
                {
                  if (expressions.Poses[GameText.FriendlySpeaker].ContainsKey(faceFileId))
                  {
                    PoseId = expressions.Poses[GameText.FriendlySpeaker][poseFileId];
                    FaceId = "f01";
                  }
                }
                else
                {
                  FaceId = expressions.Faces[GameText.FriendlySpeaker][PoseId][faceFileId];
                }
              }
              else
              {
                HasFace = true;
                FaceId = "f01";
              }
            }
            else if (poseFileId == 0 && faceFileId != 0)
            {
              HasPose = true;
              PoseId = "p01";
              if (expressions.Faces[GameText.FriendlySpeaker][PoseId].ContainsKey(faceFileId))
              {
                FaceId = expressions.Faces[GameText.FriendlySpeaker][PoseId][faceFileId];
              }
            }
          }
          else
          {
            HasPose = false;
          }
        }
      }
    }

    public override string ToString()
    {
      if (HasFace && FaceId != null)
      {
        return $"[{GameText.FriendlySpeaker}-{PoseId}-{FaceId}] {GameText.English}";
      }
      if (HasPose && PoseId != null)
      {
        return $"[{GameText.FriendlySpeaker}-{PoseId}] {GameText.English}";
      }
      return $"[{GameText.FriendlySpeaker}] {GameText.English}";
    }

    private string ReadString(BinaryReader reader)
    {
      var stringLength = reader.ReadInt32();
      // If the string is empty, we can cheap out, then bail out.
      if (stringLength == 0)
      {
        //reader.ReadInt32(); // 4 bytes for the empty string's alignment padding.
        return string.Empty;
      }
      var stringBytes = reader.ReadBytes(stringLength);
      char[] asciiCharBuffer = new char[stringLength];
      Encoding.ASCII.GetChars(stringBytes, asciiCharBuffer);
      // Ensure the stream is still aligned after the read.
      if (reader.BaseStream.Position % 4 != 0)
      {
        reader.ReadBytes(4 - (int)reader.BaseStream.Position % 4);
      }
      return new string(asciiCharBuffer);
    }
    public bool Equals(MessageClip other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return TextId == other.TextId;
    }
    
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return Equals((MessageClip)obj);
    }
    
    public override int GetHashCode()
    {
      return (TextId != null ? TextId.GetHashCode() : 0);
    }
  }

  public class MessageClipComparer : Comparer<MessageClip>
  {
    public override int Compare(MessageClip? x, MessageClip? y)
    {
      return StringComparer.Create(CultureInfo.CurrentCulture, true).Compare(x.TextId, y.TextId);
    }
  }
}
