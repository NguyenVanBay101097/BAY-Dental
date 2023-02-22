import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResGroupAccessCuDialogComponent } from './res-group-access-cu-dialog.component';

describe('ResGroupAccessCuDialogComponent', () => {
  let component: ResGroupAccessCuDialogComponent;
  let fixture: ComponentFixture<ResGroupAccessCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResGroupAccessCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResGroupAccessCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
