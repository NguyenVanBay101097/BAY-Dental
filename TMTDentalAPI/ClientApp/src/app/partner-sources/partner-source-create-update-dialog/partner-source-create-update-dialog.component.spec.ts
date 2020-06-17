import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSourceCreateUpdateDialogComponent } from './partner-source-create-update-dialog.component';

describe('PartnerSourceCreateUpdateDialogComponent', () => {
  let component: PartnerSourceCreateUpdateDialogComponent;
  let fixture: ComponentFixture<PartnerSourceCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSourceCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSourceCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
