export class ProfileResponse {
    Id: string;
    FirstName: string;
    LastName: string;
    Type: string;
    Bdate: string;
    City: string;
    PhotoUrl: string;
    Audios: number;
    Photos: number;
    Friends: number;
    Groups: number;
    MembersCount: number;
    WallItems: WallItem[];
    LikeUsers: LikeUser[];
  }
  
  class LikeUser {
    Id: number;
    OwnerId: string;
    ProfileId: string;
    FullName: string;
    PhotoUrl: string;
    LikeCount: number;
  }
  
  class WallItem {
    Id: string;
    CommentsCount: number;
    LikesCount: number;
    Text: string;
    HistoryText?: string;
    HistoryId?: any;
    Url: string;
    Type: string;
    ProfileId: string;
  }