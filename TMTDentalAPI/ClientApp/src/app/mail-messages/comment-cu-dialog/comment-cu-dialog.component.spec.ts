import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentCuDialogComponent } from './comment-cu-dialog.component';

describe('CommentCuDialogComponent', () => {
  let component: CommentCuDialogComponent;
  let fixture: ComponentFixture<CommentCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommentCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommentCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
