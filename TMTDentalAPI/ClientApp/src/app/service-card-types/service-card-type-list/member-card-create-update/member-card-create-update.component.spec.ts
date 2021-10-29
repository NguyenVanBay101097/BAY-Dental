import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberCardCreateUpdateComponent } from './member-card-create-update.component';

describe('MemberCardCreateUpdateComponent', () => {
  let component: MemberCardCreateUpdateComponent;
  let fixture: ComponentFixture<MemberCardCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MemberCardCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberCardCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
