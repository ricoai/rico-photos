import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AiObjectDetailsComponent } from './ai-object-details.component';

describe('AiObjectDetailsComponent', () => {
  let component: AiObjectDetailsComponent;
  let fixture: ComponentFixture<AiObjectDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AiObjectDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AiObjectDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
