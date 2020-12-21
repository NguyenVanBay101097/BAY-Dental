import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboBiteJointCuDialogComponent } from './labo-bite-joint-cu-dialog.component';

describe('LaboBiteJointCuDialogComponent', () => {
  let component: LaboBiteJointCuDialogComponent;
  let fixture: ComponentFixture<LaboBiteJointCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboBiteJointCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboBiteJointCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
