import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAdvanceCreateUpdateDialogComponent } from './partner-advance-create-update-dialog.component';

describe('PartnerAdvanceCreateUpdateDialogComponent', () => {
  let component: PartnerAdvanceCreateUpdateDialogComponent;
  let fixture: ComponentFixture<PartnerAdvanceCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAdvanceCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAdvanceCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
