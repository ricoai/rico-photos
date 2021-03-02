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

  lat = 32.73172;
  lng = -117.15168;
  mapType = 'satellite';
  //mapType = "roadmap";
  zoomLevel = 16;

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

  ngAfterViewInit() {

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

        // Validate the date time string
        if (this.userImage.metaData.hasOwnProperty('ExifDTOrig')) {
          this.userImage.metaData.ExifDTOrig = this.validDateFormat(this.userImage.metaData.ExifDTOrig);
        }

        console.log(this.userImage);

        // Get the latitude and longitude for the image
        if (this.userImage.metaData.hasOwnProperty('GpsLatitude')) {
          this.lat = this.userImage.metaData.GpsLatitude;
          if (this.userImage.metaData.GpsLatitudeRef == "S") {
            this.lat *= -1;
          }
        }

        if (this.userImage.metaData.hasOwnProperty('GpsLongitude')) {
          this.lng = this.userImage.metaData.GpsLongitude;
          if (this.userImage.metaData.GpsLongitudeRef == "W") {
            this.lng *= -1;
          }
        }

      });
  }

  // Fix the date time string
  // There is a space between the date and time and angular wants a T.
  // The date needs '-' and not ':' seperating them.
  validDateFormat(dateString) {
    if (dateString) {
      //dateString = dateString.replace(/\s/, 'T');

      // Replace : with - in the date
      // /: says to replace ':'
      // /g says to replace all occurances
      var date = dateString.substring(0,10).replace(/:/g, '-');
      var time = dateString.substring(11, 20);

      return date + "T" + time;
    }

    return null;

  }
}
