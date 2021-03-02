import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'app-ai-facial-details',
  templateUrl: './ai-facial-details.component.html',
  styleUrls: ['./ai-facial-details.component.css']
})
export class AiFacialDetailsComponent implements OnInit {

  // Input values
  @Input() aiFacial: any;
  @Input() s3Path: string;

  // Minimum Confidence for emotion
  public emotionConfidenceMin: number;

  // Bounding box image values
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
    this.emotionConfidenceMin = 50;

    // Create the image
    // The bounding box is given as a scalefactor,
    // so calculation the actual value.
    // Create an image element
    this.image = new Image();
    this.image.src = this.s3Path;
    this.image.onload = () => {
      this.imgWidth = this.image.width;
      this.imgHeight = this.image.height;

      // BoundingBox values are a scalefactor
      // Ex: 
      // Height: 0.5126616
      // Left: 0.3416092
      // Top: 0.31881154
      // Width: 0.25459996
      this.boundingHeight = this.imgHeight * this.aiFacial.BoundingBox.Height
      this.boundingWidth = this.imgWidth * this.aiFacial.BoundingBox.Width
      this.boundingTop = this.imgHeight * this.aiFacial.BoundingBox.Top
      this.boundingLeft = this.imgWidth * this.aiFacial.BoundingBox.Left

      // So the image
      this.showImage();
    }
  }

  // Set the image to the image element
  // Draw the rectangle on the image
  showImage() {
    // Get the canvas
    this.boundingCanvasElement = this.boundingCanvas.nativeElement;
    // Get the context as a 2D image
    this.context = this.boundingCanvasElement.getContext("2d");
    // Set the dimensions of the canvas element
    this.boundingCanvasElement.width = this.imgWidth;
    this.boundingCanvasElement.height = this.imgHeight;
    // Draw the image to the canvas
    this.context.drawImage(this.image, 0, 0, this.imgWidth, this.imgHeight);

    // Draw the rectangle on the image
    this.drawRect();
  }

  // Draw the rectangle based on the bounding settings.
  drawRect(color = "lime") {
    this.context.beginPath();
    // Draw the rectangle with the bounding settings
    this.context.rect(this.boundingLeft, this.boundingTop, this.boundingWidth, this.boundingHeight);
    // Set the width of the lines
    this.context.lineWidth = 8;
    // Set the color of the lines
    this.context.strokeStyle = color;
    // Draw the lines
    this.context.stroke();
  }

}
