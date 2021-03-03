import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  images = ["https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-face-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-object-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-text-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-upload-small.png", "https://ricoai-demo.s3-us-west-2.amazonaws.com/web/rico-ai-photo-demo-dash-small.png",];

}
