import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageFileUploadComponent } from './image-file-upload.component';

describe('ImageFileUploadComponent', () => {
  let component: ImageFileUploadComponent;
  let fixture: ComponentFixture<ImageFileUploadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImageFileUploadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImageFileUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
