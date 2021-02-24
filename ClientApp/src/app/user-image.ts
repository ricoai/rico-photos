
export interface UserImage {
  id: number;
  userId: string;
  create: Date;
  modified: Date;
  origImageName: string;
  imageName: string;
  s3Path: string;
  s3ThumbPath: string;
  fileType: string;
  metaData: string;
  tags: string[];
  albums: number[];
  aiTags: string;
}
