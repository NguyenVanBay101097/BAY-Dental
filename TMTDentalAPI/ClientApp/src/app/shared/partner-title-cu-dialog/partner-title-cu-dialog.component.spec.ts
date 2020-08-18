import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTitleCuDialogComponent } from './partner-title-cu-dialog.component';

describe('PartnerTitleCuDialogComponent', () => {
  let component: PartnerTitleCuDialogComponent;
  let fixture: ComponentFixture<PartnerTitleCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerTitleCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTitleCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
