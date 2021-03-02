import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'app-ai-facial-details',
  templateUrl: './ai-facial-details.component.html',
  styleUrls: ['./ai-facial-details.component.css']
})
export class AiFacialDetailsComponent implements OnInit {

  @Input() aiFacial: any;
  @Input() s3Path: string;

  public emotionConfidenceMin: number;

  public image: HTMLImageElement;
  public imgWidth: number;
  public imgHeight: number;
  public boundingTop: number;
  public boundingLeft: number;
  public boundingWidth: number;
  public boundingHeight: number;
  private context: CanvasRenderingContext2D;
  private boundingCanvasElement: any;
  @ViewChild("boundingLayer", { static: false }) boundingCanvas: ElementRef;

  constructor() { }

  ngOnInit() {

    // Minimum value to show the emotional value
    this.emotionConfidenceMin = 75;

    this.image = new Image();
    this.image.src = this.s3Path;
    this.image.onload = () => {
      this.imgWidth = this.image.width;
      this.imgHeight = this.image.height;

      // BoundingBox values are a scalefactor
      // Height: 0.5126616
      // Left: 0.3416092
      // Top: 0.31881154
      // Width: 0.25459996
      this.boundingHeight = this.imgHeight * this.aiFacial.BoundingBox.Height
      this.boundingWidth = this.imgWidth * this.aiFacial.BoundingBox.Width
      this.boundingTop = this.imgHeight * this.aiFacial.BoundingBox.Top
      this.boundingLeft = this.imgWidth * this.aiFacial.BoundingBox.Left

      this.showImage();
    }
  }

  showImage() {
    this.boundingCanvasElement = this.boundingCanvas.nativeElement;
    this.context = this.boundingCanvasElement.getContext("2d");
    this.boundingCanvasElement.width = this.imgWidth;
    this.boundingCanvasElement.height = this.imgHeight;
    this.context.drawImage(this.image, 0, 0, this.imgWidth, this.imgHeight);

    this.drawRect();
  }

  drawRect(color = "lime") {
    this.context.beginPath();
    this.context.rect(this.boundingLeft, this.boundingTop, this.boundingWidth, this.boundingHeight);
    this.context.lineWidth = 8;
    this.context.strokeStyle = color;
    this.context.stroke();
  }

}
