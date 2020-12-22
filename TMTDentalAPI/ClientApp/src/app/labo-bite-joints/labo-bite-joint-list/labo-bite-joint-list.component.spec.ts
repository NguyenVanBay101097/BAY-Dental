import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboBiteJointListComponent } from './labo-bite-joint-list.component';

describe('LaboBiteJointListComponent', () => {
  let component: LaboBiteJointListComponent;
  let fixture: ComponentFixture<LaboBiteJointListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboBiteJointListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboBiteJointListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
