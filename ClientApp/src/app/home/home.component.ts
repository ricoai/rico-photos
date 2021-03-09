import { Component } from '@angular/core';
import { UserImageService } from '../user-image.service';
import { UserImage } from '../user-image';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  images = ["https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-face-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-object-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-text-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-upload-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-dash-small.png",];

  public error: string = ""
  userImages: any = [];

  constructor(private userImageService: UserImageService) {}

  async ngOnInit() {

    try {
      console.log("Test database connection");
      this.userImageService.testDbConnection().subscribe(result => {
        this.userImages = result;

        console.log(result);
      });
    } catch (err) {
      this.error = err.message;
      console.log(err);
    }
  }
}
