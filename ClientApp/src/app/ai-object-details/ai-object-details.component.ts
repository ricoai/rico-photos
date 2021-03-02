import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'app-ai-object-details',
  templateUrl: './ai-object-details.component.html',
  styleUrls: ['./ai-object-details.component.css']
})
export class AiObjectDetailsComponent implements OnInit {

  @Input() objects: any;
  @Input() s3Path: string;

  public listBB: Array<any> = [];

  // Bounding box image values
  public image: HTMLImageElement;
  public imgWidth: number;
  public imgHeight: number;
  private context: CanvasRenderingContext2D;
  private boundingCanvasElement: any;
  @ViewChild("boundingLayer", { static: false }) boundingCanvas: ElementRef;

  constructor() { }

  ngOnInit() {

    // Get all the bounding boxes from the objects
    this.getAllBoundingBoxes();

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
      //this.boundingHeight = this.imgHeight * this.aiFacial.BoundingBox.Height
      //this.boundingWidth = this.imgWidth * this.aiFacial.BoundingBox.Width
      //this.boundingTop = this.imgHeight * this.aiFacial.BoundingBox.Top
      //this.boundingLeft = this.imgWidth * this.aiFacial.BoundingBox.Left

      // So the image with the rectangles
      this.showImage();

      this.drawBoundingBoxes();
    }
  }

  // Get all the bounding boxes from the given objects.
  getAllBoundingBoxes() {
    var x, i;
    for (x = 0; x < this.objects.length; x++) {
      for (i = 0; i < this.objects[x].Instances.length; i++) {
        this.listBB.push(this.objects[x].Instances[i].BoundingBox);
      }
    }
  }

  // Bounding boxes are given as a scale factor.
  // Recalculate the bounding boxes using the actual
  // image width and height.  Then draw it to the image.
  drawBoundingBoxes() {
          // BoundingBox values are a scalefactor
      // Ex: 
      // Height: 0.5126616
      // Left: 0.3416092
      // Top: 0.31881154
      // Width: 0.25459996
    var x;
    for (x = 0; x < this.listBB.length; x++) {
      // Recalculate the bounding box
      var height = this.imgHeight * this.listBB[x].Height;
      var width = this.imgWidth * this.listBB[x].Width;
      var top = this.imgHeight * this.listBB[x].Top;
      var left = this.imgWidth * this.listBB[x].Left;

      // Draw the rectangle
      this.drawRect(left, top, width, height)

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
  }

  // Draw the rectangle based on the bounding settings.
  drawRect(left, top, width, height, color = "lime") {
    this.context.beginPath();
    // Draw the rectangle with the bounding settings
    this.context.rect(left, top, width, height);
    // Set the width of the lines
    this.context.lineWidth = 8;
    // Set the color of the lines
    this.context.strokeStyle = color;
    // Draw the lines
    this.context.stroke();
  }

}
