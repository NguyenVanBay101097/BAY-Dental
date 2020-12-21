import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboBridgeCuDialogComponent } from './labo-bridge-cu-dialog.component';

describe('LaboBridgeCuDialogComponent', () => {
  let component: LaboBridgeCuDialogComponent;
  let fixture: ComponentFixture<LaboBridgeCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboBridgeCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboBridgeCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
