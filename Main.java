...
	private static final Area SAWMILL = new Area(3300, 3487, 3305, 3491);
	private static final Area BANK = new Area(3250, 3420, 3254, 3424);
	Rectangle[] rs = { new Rectangle(150, 80, 65, 65), new Rectangle(300, 80, 65, 65), new Rectangle(150, 215, 65, 65), new Rectangle(300, 215, 65, 65) };
	Color chatBoxColor = new Color(0xD8C49D);
	public boolean running = false;
	long nextRotate;
	long nextRotate2;
	Timer timer;
	
	int logsId;
	int plankCost;
	String logType;
	String plankType;
	Filter<Item> qualifiedLogs = i -> i != null && i.getName().equals(logType);
	Filter<Item> qualifiedPlanks = i -> i != null && i.getName().equals(plankType);
	int planksMade;
	NPC sawmillOperator;
	Item coins;
	
	public enum State {
		WALKTOSAWMILL, BUYPLANKS, RETURNTOBANK, DEPOSITPLANKS, LOGOUT
	}
	
	private State getState() {
		if(getInventory().contains(qualifiedLogs)) {
			coins = getInventory().get("Coins");
			if(getInventory().contains(coins)) {
				if(coins.getAmount() >= plankCost) {
					if(getLocalPlayer().distance(SAWMILL.getCenter()) > 6) {
						return State.WALKTOSAWMILL;
					}
					else {
						return State.BUYPLANKS;
					}
				}
				else {
					return State.LOGOUT;
				}
			}
			else {
				return State.LOGOUT;
			}
		}
		else if(getInventory().contains(qualifiedPlanks) || getInventory().onlyContains("Coins")) {
			if(getLocalPlayer().distance(BANK.getCenter()) > 6) {
				return State.RETURNTOBANK;
			}
			else {
				if(getBank().isOpen()) {
					if(getBank().contains(qualifiedLogs) && getBank().get(qualifiedLogs).getAmount() > 1) {
						return State.DEPOSITPLANKS;
					}
					else {
						return State.LOGOUT;
					}
				}
				else {
					return State.DEPOSITPLANKS;
				}
			}
		}
		else {
			return State.LOGOUT;
		}
	}

	public void walkToSawmill() {
		if(System.currentTimeMillis() > nextRotate)
		{
			getCamera().rotateTo(Calculations.random(0, 2000), Calculations.random(150, 350));
			nextRotate = System.currentTimeMillis() + Calculations.random(3000, 6000);
		}
		if(getWalking().getDestinationDistance() < Calculations.random(2, 7)) {
			getWalking().walk(SAWMILL.getRandomTile());
		}
	}

	public void buyPlanks() {
		if(getShop().isOpen()) {
			if(getShop().close()) {
				sleepUntil(() -> !getShop().isOpen(), Calculations.random(1500, 3500));
			}
		}
		if(getWidgets().getWidget(403) != null && getWidgets().getWidget(403).isVisible()) {
			if(getClient().getMenu().isMenuVisible()) {
				if(getClient().getMenu().clickIndex(4)) {
					sleepUntil(() -> getInventory().contains(qualifiedPlanks), Calculations.random(1500, 3500));
					planksMade += getInventory().count(qualifiedPlanks);
				}
			}
			else {
				if(getMouse().click(rs[logsId], true)) {
					sleepUntil(() -> getClient().getMenu().isMenuVisible(), Calculations.random(1500, 3500));
				}
			}
		}
		else {
			sawmillOperator = getNpcs().closest("Sawmill operator");
			if(sawmillOperator != null) {
				if(sawmillOperator.interactForceRight("Buy-plank")) {
					sleepUntil(() -> getDialogues().inDialogue(), Calculations.random(1500, 3500));
				}
			}
		}
	}
	
	public void returnToBank() {
		if(System.currentTimeMillis() > nextRotate2)
		{
			getCamera().rotateTo(Calculations.random(0, 2000), Calculations.random(150, 350));
			nextRotate2 = System.currentTimeMillis() + Calculations.random(3000, 6000);
		}
		if(getWalking().getDestinationDistance() < Calculations.random(2, 7)) {
			getWalking().walk(BANK.getRandomTile());
		}
	}
	
	public void depositPlanks() {
		if(getBank().isOpen()) {
			if(getInventory().contains(qualifiedPlanks)) {
				if(getBank().depositAll(qualifiedPlanks)) {
					sleepUntil(() -> !getInventory().contains(qualifiedPlanks), Calculations.random(1500, 3500));
				}
			}
			else {
				if(getBank().withdrawAll(qualifiedLogs)) {
					sleepUntil(() -> getInventory().contains(qualifiedLogs), Calculations.random(1500, 3500));
				}
			}
		}
		else {
			if(getBank().openClosest()) {
				sleepUntil(() -> getBank().isOpen(), Calculations.random(1500, 3500));
			}
		}
	}
	
	public void logout() {
		if(getBank().isOpen()) {
			getBank().close();
		}
		getTabs().logout();
	}

	public int onLoop() {
		if(!running)
			return 200;
		log("" + getState());
		switch(getState()) {
		case WALKTOSAWMILL:
			walkToSawmill();
			break;
		case BUYPLANKS:
			buyPlanks();
			break;
		case RETURNTOBANK:
			returnToBank();
			break;
		case DEPOSITPLANKS:
			depositPlanks();
			break;
		case LOGOUT:
			logout();
			break;
		}
		return 0;
	}
	
	public void onStart() {
		new Form(this).setVisible(true);
		timer = new Timer();
		getWalking().setRunThreshold(Calculations.random(4, 13));
	}
...
