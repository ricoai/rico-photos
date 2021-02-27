import { HttpEventType } from '@angular/common/http';
import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserImage } from "../user-image";
import { UserImageService } from '../user-image.service';

@Component({
  selector: 'app-image-details',
  templateUrl: './image-details.component.html',
  styleUrls: ['./image-details.component.css']
})
export class ImageDetailsComponent implements OnInit {

  // Take the User Image as an input to display all the details of the Images
  // UserImage contains all the information about the image.
  //@Input() userImage: UserImage;

  userImage: UserImage;

  constructor(private route: ActivatedRoute, private userImageService: UserImageService) { }

  ngOnInit() {
    // Get the Image ID
    const imageId = this.route.snapshot.paramMap.get('id');

    // Get the user image details
    //this.userImageService.getUserImage(imageId)
    //  .subscribe(result => {
    //    this.userImage = result as UserImage;
    //    console.log(event);
    //  });

    this.getUserImage(imageId);

  }

  getUserImage(imageId) {
    this.userImageService.getUserImage(imageId)
      .subscribe(result => {
        // Set the User Image
        this.userImage = result as UserImage

        // Convert the JSON strings to JSON objects
        this.userImage.metaData = JSON.parse(this.userImage.metaData);
        this.userImage.aiFacialTags = JSON.parse(this.userImage.aiFacialTags);
        this.userImage.aiModerationTags = JSON.parse(this.userImage.aiModerationTags);
        this.userImage.aiObjectsTags = JSON.parse(this.userImage.aiObjectsTags);
        this.userImage.aiTextInImageTags = JSON.parse(this.userImage.aiTextInImageTags);

        console.log(this.userImage);
      });
  }

}
